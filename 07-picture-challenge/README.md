# Challenge 6: Durable Pattern

![St. Nicholas challenge](https://res.cloudinary.com/jen-looper/image/upload/v1575132446/images/challenge-6_qpqesc.jpg)

## Solution

New things were learnt today. Never experimented with **Slack Apps** or **Durable Functions** before.

Resources used:

| Resource | Description     |
| :------------- | :------------- |
| [**Build a Slash Command**](https://api.slack.com/tutorials/slash-block-kit)                                                | Learn how to build a `/slash` command for Slack                 |
| [**Slack Incoming Webhooks**](https://api.slack.com/messaging/webhooks)                                                     | Learn what incoming message webhooks are and how to use them    |
| [**Stateful Serverless**](https://dev.to/azure/stateful-serverless-with-durable-functions-2jff)                             | Learn how to create schedules and timers with Durable Functions |
| [**Durable Contraints**](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-code-constraints) | Beware of non-deterministic functions                           |
| [**Chrono**](https://github.com/wanasit/chrono) | Convert natural English language to date/time |
| [**Moment Timezone**](https://github.com/moment/moment-timezone) | Handle timezones correctly from JS dates |

![Santa's Scheduler screenshot](images/slack.png)

## Happy St. Nicholas Day!

Here in the Styrian region of Austria, it's said that today is the day that St. Nicholas goes around handing out presents, while his evil counterpart Krampus whips those who have been naughty. These days, that mostly results in people giving each other bundles of _ruten_, bundles of birch twigs that have been painted gold.

You're supposed to hang up these ruten year-round to remind children to be good, but of course today's children don't spend much more time in online chats than sitting in front of the fireplace. Let's write a reminder tool using serverless tech that lets Austrian children set reminders to do good deeds in their favorite chat app!

Build a chat integration for your favorite chat service (e.g. Discord or Slack) that lets you schedule tasks using natural language (e.g. `/schedule volunteer at the senior citizens' center tomorrow at 11:00`). You should be able to get a confirmation that your event has been scheduled, and then get a notification at the correct time.
