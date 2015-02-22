var express = require('express');
var router = express.Router();
var http = require("http");
var debug = require('debug')('tracy');

/* GET entries page. */
router.get('/', function(req, res, next) {
    http.get("http://localhost:8801/GetEntryList", function (httpRes) {
        var data = "";
        httpRes.on("data", function (chunk) {
            data += chunk;
        });
        httpRes.on("end", function () {
            var entries = JSON.parse(data);
            res.render('entries', { title: 'My Entries', entries: entries });
        });
    });
});

module.exports = router;
