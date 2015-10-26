var express = require('express');
var router = express.Router();
var http = require("http");
var debug = require('debug')('tracy');
var request = require('request');

/* GET resources page. */
router.get('/list/:entryId/:entryName', function(req, res, next) {
    request('http://localhost:8801/GetMediaFileList?entryId=' + req.params.entryId + '&sessionId=' + req.cookies.sessionId, function (error, response, body) {
        if (!error && response.statusCode == 200) {
            var fileData = JSON.parse(body);
            var fileList = processMediaFiles(fileData.result);
            request('http://localhost:8801/GetResourceList?entryId=' + req.params.entryId, function (error, response, body) {
                if (!error && response.statusCode == 200) {
                    var resData = JSON.parse(body);
                    res.render('resources', { entryId: req.params.entryId, title: req.params.entryName, resources: resData.result, userMediaFiles: fileList });
                }
            });
        }
    });
});

function getWeight(episode) {
    if (episode == 'other') return -1;
    return parseInt(episode); 
}

function processMediaFiles(rawFiles){
    var dict = {};
    if(!rawFiles) return dict;
    //convert to dict, use episode as key
    rawFiles.forEach(function (rawFile) {
        var key = (rawFile.mediaFile.episode != null) ? rawFile.mediaFile.episode : 'other';
        var list = dict[key];
        if (!list) {
            list = [];
            dict[key] = list;
        }
        list.push(rawFile);
    }, this);
    
    //sort by episode
    var keyList = Object.keys(dict);
    keyList.sort(function (a, b) {
        return getWeight(a) - getWeight(b);
    });
    
    //convert to list
    var rtn = [];
    var firstNewFlag = true;
    keyList.forEach(function (key) {
        var files = dict[key];
        //if all sub items are new, this episode has new flag
        var isNew = true;
        for (var i = 0; i < files.length; i++) {
            if (!files[i].isNew) {
                isNew = false;
                break;
            }
        }
        var willExpand = false;
        if (isNew && firstNewFlag) {
            firstNewFlag = false;
            willExpand = true;
        }
        rtn.push({ 
            episode: key, 
            files: files,
            isNew: isNew,
            willExpand: willExpand 
        });
    });
    return rtn;
}

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
            url: 'http://localhost:8801/DownloadResource?entryId=' + param.entryId + '&resourceId=' + param.resourceId,
            json: param
        }, function (error, response, body) {
            res.send(body);
        });
    } else {
        res.send(rtn);
    }
});

router.get('/download/:mediaFileId', function(req, res, next) {
    request('http://localhost:8801/GetDownloadUrl?mediaFileId=' + req.params.mediaFileId + '&sessionId=' + req.cookies.sessionId, function (error, response, body) {
        if (!error && response.statusCode == 200) {
            var url = JSON.parse(body);
            if(url && url.result){
                res.redirect(url.result);
            }else{
                res.send("Failed to get download url.");
            }
        }
    });
});

module.exports = router;
