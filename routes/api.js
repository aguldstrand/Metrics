var Logger = require('../utils/logger');

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

	app.get('/api/stats/:date', function(request, response) {
		response.end();
	});
};
