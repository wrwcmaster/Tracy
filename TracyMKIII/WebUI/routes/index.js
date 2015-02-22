var express = require('express');
var router = express.Router();
var http = require("http");
var debug = require('debug')('tracy');

/* GET home page. */
router.get('/', function(req, res, next) {
    res.redirect('/entries');
});

module.exports = router;
