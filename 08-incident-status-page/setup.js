const client = require('./db.js');

const databaseDefinition = { id: "santas-rds-db" };
const collectionDefinition = { id: "status" };

const setupAndSeedDatabase = async ()  => {

  const { database: db } = await client.databases.create(databaseDefinition);
  console.log('Database created.');

  const { container } = await db.containers.create(collectionDefinition);
  console.log('Collection created.');

  await container.items.create({
    "id": "e0eb6e85-176d-4ce6-89ae-1f699aaa0bab",
    "system": "Reindeer Guidance",
    "status": "Closed",
  });

  await container.items.create({
    "id": "ebe2e863-bf84-439a-89f8-39975e7d6766",
    "system": "Delivery System",
    "status": "Closed",
  });

  console.log('Seed data added.');
};

setupAndSeedDatabase().catch(err => {
  console.error('Error setting up database:', err);
});