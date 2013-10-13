var fs = require('fs');
var path = require('path');
var moment = require('moment');

function Logger(instanceId) {
	this.instanceId = instanceId;
}

Logger.prototype = {
	log: function(data) {
		var time = moment();
		var filePath = path.join('logs', time.format('YYYY-MM-DD') + '.' + this.instanceId + '.log');

		fs.appendFile(filePath, JSON.stringify({
			t: time.format('HH:mm:ss.SSS'),
			d: data
		}) + '\n',
		function(err) {
			// Do not wait for the data to be written to file
			if(err) {
				console.error(err);
			}
		});
	}
};

module.exports = Logger;