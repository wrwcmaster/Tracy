var express = require('express');
var router = express.Router();
var http = require("http");
var debug = require('debug')('tracy');
var request = require('request');

/* GET resources page. */
router.get('/:entryId/:entryName', function(req, res, next) {
    request('http://localhost:8801/GetResourceList?entryId=' + req.params.entryId, function (error, response, body) {
        if (!error && response.statusCode == 200) {
            var data = JSON.parse(body);
            res.render('resources', { title: req.params.entryName, resources: data.result });
        }
    });
});

module.exports = router;
