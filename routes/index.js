
/*
* GET home page.
*/

exports.index = function(req, res){
	res.render('index', {
		title: 'Express',
		questions: [{
			text: 'bla blra',
			value: Math.random() * 100
		}, {
			text: 'asdklö aölsd',
			value: Math.random() * 100
		}, {
			text: 'asdklö aölsd',
			value: Math.random() * 100
		}, {
			text: 'asdklö aölsd',
			value: Math.random() * 100
		}, {
			text: 'asdklö aölsd',
			value: Math.random() * 100
		}, {
			text: 'asdklö aölsd',
			value: Math.random() * 100
		}, {
			text: 'asdklö aölsd',
			value: Math.random() * 100
		}]
	});
};