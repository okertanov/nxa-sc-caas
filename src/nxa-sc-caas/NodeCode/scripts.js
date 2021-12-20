module.exports = {
    uuid: function (callback) {
    let uuidRes = function () {
        const uuidv4 = require('uuid').v4;
        let retUuid = {
            uuid: uuidv4()
        };
        return retUuid;
    }
    callback(null, uuidRes());
},
    momentjs: function (callback, timestamp) 
    {
        let momentJsRes = function () {
            let retMoment = {
                dateString: require('moment').unix(timestamp).format('YYYY-MM-DD')
            };
            return retMoment;
        }
        callback(null, momentJsRes());
    },
    bcrypt: function (callback, message) 
    {
        let bcryptRes = function () {
            let retHash = {
                message: message,
                hash: require('bcrypt').hashSync(message, 10)
            };
            return retHash;
        }
        callback(null, bcryptRes());
    },
    evaluate: function (callback, codeToEval) {
        let evalRes = function () {
            let retVal =
                { result: eval(codeToEval)}
            return retVal;
        }
        callback(null, evalRes());
    }
};