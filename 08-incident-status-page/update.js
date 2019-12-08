const client = require('./db.js');

const databaseDefinition = { id: "santas-rds-db" };
const collectionDefinition = { id: "status" };

const init = async () => {
  const { database } = await client.databases.createIfNotExists(databaseDefinition);
  const { container } = await database.containers.createIfNotExists(collectionDefinition);
  return { database, container };
}

function sleep(miliseconds) {
  var currentTime = new Date().getTime();

  while (currentTime + miliseconds >= new Date().getTime()) {
  }
}

const updateStatus = async (id, status) => {
  const { container } = await init();

  const doc = await container.item(id);

  const { body: existingStatus } = await doc.read();

  Object.assign(existingStatus, {
    "status": status
  });

  await doc.replace(existingStatus);

  console.log(`Data updated: ${JSON.stringify(existingStatus)}`);
};

const updateData = async () => {

  console.log('Update the status of the Reindee Guidance system to Ongoing');
  await updateStatus('e0eb6e85-176d-4ce6-89ae-1f699aaa0bab', 'Ongoing');
  sleep(5000);

  console.log('Update the status of the Delivery system to Open');
  await updateStatus('ebe2e863-bf84-439a-89f8-39975e7d6766', 'Open');
  sleep(5000);

  console.log('Update the status of the Reindee Guidance system to Open');
  await updateStatus('e0eb6e85-176d-4ce6-89ae-1f699aaa0bab', 'Open');
  sleep(5000);

  console.log('Update the status of the Delivery system to Closed');
  await updateStatus('ebe2e863-bf84-439a-89f8-39975e7d6766', 'Closed');
  sleep(5000);

  console.log('Update the status of the Reindee Guidance system to Closed');
  await updateStatus('e0eb6e85-176d-4ce6-89ae-1f699aaa0bab', 'Closed');
};

updateData().catch(err => {
  console.error(err);
});