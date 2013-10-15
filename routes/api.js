var Logger = require('../utils/logger');
var store = require('../utils/store');

exports.init = function (instanceId, app) {
	console.log('api init');
	var logger = new Logger(instanceId);

	app.get('/api/track', function(request, response) {
		logger.log(request.query);
		response.end();
	});

	app.post('/api/track', function(request, response) {
		logger.log(request.body);
		response.end();
	});

	app.get('/api/stats', function(request, response) {

		store.loadDateRange(request.query.from, request.query.to, function(err, datas) {
			if(err) {
				response.status(500);
				response.send(err);
				return;
			}

			response.send(datas);
		});
	});
};
