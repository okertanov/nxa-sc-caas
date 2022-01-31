module.exports = {
    uuid: (callback) => {
        const uuidRes = function () {
        const uuidv4 = require('uuid').v4;
            const retUuid = {
            uuid: uuidv4()
        };
        return retUuid;
    }
    callback(null, uuidRes());
},
    momentjs: (callback, timestamp) => {
        const momentJsRes = function () {
            const retMoment = {
                dateString: require('moment').unix(timestamp).format('YYYY-MM-DD')
            };
            return retMoment;
        }
        callback(null, momentJsRes());
    },
    bcrypt: (callback, message) => {
        const bcryptRes = function () {
            const retHash = {
                message: message,
                hash: require('bcrypt').hashSync(message, 10)
            };
            return retHash;
        }
        callback(null, bcryptRes());
    },
    evaluate: (callback, codeToEval) => {
        const evalRes = function () {
            const retVal =
                { result: eval(codeToEval)}
            return retVal;
        }
        callback(null, evalRes());
    },
    hardhatCompile: (callback, contractName, taskPath) => {
        process.chdir(`./${taskPath}`)
        const hre = require('hardhat');
        hre.run('compile').then(() => {
            hre.artifacts.readArtifact(contractName).then(artifact => {
                callback(null, artifact);
            })
                .catch(e => {
                    console.error(e);
                    callback(e);
                });
            })
            .catch(e => {
                console.error(e);
                callback(e);
            })
            .finally(() => {
                process.chdir(`../`)
            });
    }
};