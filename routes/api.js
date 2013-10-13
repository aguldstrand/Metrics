exports.init = function (instanceId, app) {
	var logger = new require('../utils/logger')(instanceId);

	app.post('api/track', function(request, response) {
		logger.log(request.body);
		response.end();
	});

	app.get('api/stats/:date', function(request, response) {
		response.end();
	});
};