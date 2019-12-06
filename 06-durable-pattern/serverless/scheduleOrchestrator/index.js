const df = require('durable-functions');

module.exports = df.orchestrator(function* (context) {
  const input = context.df.getInput()

  yield context.df.createTimer(new Date(input.startAt))

  return yield context.df.callActivity('sendToSlack', input);
});