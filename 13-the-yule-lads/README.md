# Challenge 13: The Yule Lads

![Challenge 13: The Yule Lads](https://res.cloudinary.com/jen-looper/image/upload/v1575988577/images/challenge-13_o3hrxg.jpg)

## Solution

Python **Azure Function** with a blob trigger to generate a joke when the model is uploaded. Plus another with an HTTP trigger so that a joke can be created with the model which is already uploaded to the blob storage container. With the help of - [Build your jokes generator using Machine Learning and Serverless](https://dev.to/azure/build-your-jokes-generator-using-machine-learning-and-serverless-5g4a)

To generate the model, run `generate-model.py` in the `test` folder. The resulting `hdf5` file can then be uploaded to the blob storage container to trigger the function.

* Joke from the HTTP triggered function
![Joke from the HTTP triggered function](screenshot/joke.png)

## The Challenge

Here in Iceland, the holidays are full of tricksy traditions. For the thirteen days leading up to Christmas, it's said that children are visited each night by one of the thirteen Yule Lads (Jólasveinar), a motley crew of trolls with their own distinct personalities. Kertasníkir, the Candle-Stealer, follows children and steals their candles, whereas Þvörusleikir, the Spoon-Licker, loves to steal wooden spoons and lick the food off of them. And the big scary Christmas cat Jólakötturinn will devour any children who haven't gotten a new piece of clothing as a Christmas gift!

Each night, children put out their shoes by the window. When that night's visiting Yule Lad shows up, they leave gifts in the shoes of nice girls and boys, and rotting potatoes for the naughty ones.

This year, the thirteen tricksters are ready to get a bit more tech-savvy! They'd like to use machine learning to generate jokes they can say to children. They have a big book of jokes to train a machine learning model to generate a new joke for them every time they need one. Use this [sample dataset](https://raw.githubusercontent.com/simonaco/25daysofserverless/master/jokes.txt) extracted from [icanhazdadjoke](https://icanhazdadjoke.com/api) api as a blob trigger to create a troll joke generator for our tricksters.
