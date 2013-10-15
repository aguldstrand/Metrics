var fs = require('fs');
var async = require('async');

module.exports.loadDateRange = function(fromDate, toDate, callback) {
	var basePath = 'logs';

	function filter(path, callback) {

		if(!path) {
			callback(false);
			return;
		}

		var date = path.substr(0, 10);
		if(date < fromDate || date > toDate) {
			callback(false);
			return;
		}

		fs.stat(basePath + '/' + path, function(err, stats) {
			if(err) {
				callback(false);
				return;
			}

			callback(stats.isFile());
		});
	}

	function readFile(path, callback) {
		fs.readFile(basePath + '/' + path, {
			encoding: 'utf-8'
		}, callback);
	}

	async.waterfall([
		function(callback) {
			fs.readdir(basePath, callback);
		},
		function(files, callback) {
			async.filter(files, filter, function(files) {
				callback(null, files);
			});
		},
		function(files, callback) {
			async.map(files, readFile, callback);
		},
		function(fileContents, callback) {
			var outp = [];
			for (var i = 0; i < fileContents.length; i++) {
				var fileContent = fileContents[i];

				var lines = fileContent.split('\n');
				for (var j = 0; j < lines.length; j++) {
					if(lines[j].length) {
						var data = JSON.parse(lines[j]);
						outp.push(data);
					}
				};
			};

			outp.sort(function(a, b) {
				return a.t.localeCompare(b.t);
			});

			callback(null, outp);
		}],
		callback);
};