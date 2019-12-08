module.exports = async function (context, documents) {
    const updates = documents.map(status => ({
        target: 'updated',
        arguments: [status]
    }));

    context.bindings.signalRMessages = updates;
    context.done();
}
