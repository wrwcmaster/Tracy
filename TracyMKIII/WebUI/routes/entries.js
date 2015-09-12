var express = require('express');
var router = express.Router();
//var http = require("http");
var debug = require('debug')('tracy');
var request = require('request');

/* GET entries page. */
router.get('/', function(req, res, next) {
    request('http://localhost:8801/GetEntryList', function (error, response, body) {
        if (!error && response.statusCode == 200) {
            var data = JSON.parse(body);
            res.render('entries', { title: 'My Entries', entries: data.result });
        }
    });
});

/* API */
router.post('/add', function(req, res, next) {
    console.log('entries/add');
    console.log(req.body);
    res.set('Content-Type', 'application/json');
    
    request({
        method: 'POST',
        url: 'http://localhost:8801/AddEntry',
        json: req.body
    }, function (error, response, body) {
        var rtn = {};
        rtn.errorCode = 0;
        rtn.entry = body;
        res.send(rtn);
    });
});

module.exports = router;
