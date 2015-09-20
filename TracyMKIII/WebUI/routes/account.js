var express = require('express');
var router = express.Router();

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
        rtn.errorMessage = "Password.";
    }
    
    if (rtn.errorCode == 0) {
        request({
            method: 'POST',
            url: 'http://localhost:8801/Register,
            json: param
        }, function (error, response, body) {
            res.send(body);
        });
    } else {
        res.send(rtn);
    }
});

module.exports = router;
