var express = require('express');
var router = express.Router();
var request = require('request');
/* GET page. */
router.get('/register', function(req, res, next) {
    res.render('register', { });
});
router.get('/login', function(req, res, next) {
    res.render('login', { });
});

/* API */
router.post('/register', function(req, res, next) {
    console.log('account/register');
    console.log(req.body);
    res.set('Content-Type', 'application/json');
    var param = req.body;
    var rtn = {};
    rtn.errorCode = 0;
    
    // Validate input
    if (!param.userName) {
        rtn.errorCode = 401;
        rtn.errorMessage = "Entry name cannot be empty.";
    }
    if (!param.password) {
        rtn.errorCode = 401;
        rtn.errorMessage = "Password cannot be empty.";
    }
    if (param.password != param.retypePassword) {
        rtn.errorCode = 401;
        rtn.errorMessage = "Passwords not match.";
    }
    
    if (rtn.errorCode == 0) {
        request({
            method: 'POST',
            url: 'http://localhost:8801/Register',
            json: param
        }, function (error, response, body) {
            res.send(body);
        });
    } else {
        res.send(rtn);
    }
});

router.post('/login', function(req, res, next) {
    console.log('account/login');
    console.log(req.body);
    res.set('Content-Type', 'application/json');
    var param = req.body;
    var rtn = {};
    rtn.errorCode = 0;
    
    // Validate input
    if (!param.userName) {
        rtn.errorCode = 401;
        rtn.errorMessage = "Entry name cannot be empty.";
    }
    if (!param.password) {
        rtn.errorCode = 401;
        rtn.errorMessage = "Password cannot be empty.";
    }
    
    if (rtn.errorCode == 0) {
        request({
            method: 'POST',
            url: 'http://localhost:8801/Login',
            json: param
        }, function (error, response, body) {
            if (body.result) {
                res.cookie('sessionId', body.result);
            }
            res.send(body);
        });
    } else {
        res.send(rtn);
    }
});

module.exports = router;
