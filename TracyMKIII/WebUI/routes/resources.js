var express = require('express');
var router = express.Router();
var http = require("http");
var debug = require('debug')('tracy');

/* GET resources page. */
router.get('/:entryId/:entryName', function(req, res, next) {
    http.get("http://localhost:8801/GetResourceList?entryId=" + req.params.entryId, function (httpRes) {
        var data = "";
        httpRes.on("data", function (chunk) {
            data += chunk;
        });
        httpRes.on("end", function () {
            var resources = JSON.parse(data);
            res.render('resources', { title: req.params.entryName, resources: resources });
        });
    });
    
});

module.exports = router;
