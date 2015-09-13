var express = require('express');
var router = express.Router();
var http = require("http");
var debug = require('debug')('tracy');
var request = require('request');

/* GET resources page. */
router.get('/list/:entryId/:entryName', function(req, res, next) {
    
    request('http://localhost:8801/GetMediaFileList?entryId=' + req.params.entryId, function (error, response, body) {
        if (!error && response.statusCode == 200) {
            var fileData = JSON.parse(body);
            request('http://localhost:8801/GetResourceList?entryId=' + req.params.entryId, function (error, response, body) {
                if (!error && response.statusCode == 200) {
                    var resData = JSON.parse(body);
                    res.render('resources', { entryId: req.params.entryId, title: req.params.entryName, resources: resData.result, mediaFiles: fileData.result });
                }
            });
        }
    });
    
});

/* API */
router.post('/validateEntry', function(req, res, next) {
    console.log('resources/validateEntry');
    console.log(req.body);
    res.set('Content-Type', 'application/json');
    var param = req.body;
    var rtn = {};
    rtn.errorCode = 0;
    
    // Validate input
    if (!param.name) {
        rtn.errorCode = 401;
        rtn.errorMessage = "Entry name cannot be empty.";
    }
    
    if (rtn.errorCode == 0) {
        // Check matched resources
        param.sampleCount = 10;
        request('http://localhost:8801/CheckMatchedResources?keywords=' + encodeURIComponent(param.searchKeywords) + 
                '&regExpr=' + encodeURIComponent(param.regExpr) +
                '&sampleCount=' + param.sampleCount, 
                function (error, response, body) {
                    res.send(body);
                });
    } else {
        res.send(rtn);
    }
});

router.post('/offlineDownload', function(req, res, next) {
    console.log('resources/offlineDownload');
    console.log(req.body);
    res.set('Content-Type', 'application/json');
    var param = req.body;
    var rtn = {};
    rtn.errorCode = 0;
    
    if (rtn.errorCode == 0) {
        request({
            method: 'POST',
            url: 'http://localhost:8801/DownloadResource?entryId=' + param.entryId + '&resourceId=' + param.resourceId 
        }, function (error, response, body) {
            res.send(body);
        });
    } else {
        res.send(rtn);
    }
});

router.get('/download/:mediaFileId', function(req, res, next) {
    request('http://localhost:8801/GetDownloadUrl?mediaFileId=' + req.params.mediaFileId, function (error, response, body) {
        if (!error && response.statusCode == 200) {
            var url = body;
            res.redirect(url);
        }
    });
});

module.exports = router;
