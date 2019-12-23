import azure.functions as func
from textgenrnn import textgenrnn
import tempfile

import urllib.request
import os
import logging


def main(req: func.HttpRequest) -> func.HttpResponse:
    logging.info('Python HTTP trigger function processed a request.')

    tempFilePath = tempfile.gettempdir()
    jokesfile = os.path.join(tempFilePath, "jokes.hdf5")
    urllib.request.urlretrieve(os.environ["TEXTGENRNN_MODEL_URI"], jokesfile)
    textgen = textgenrnn(jokesfile)
    joke = textgen.generate(return_as_list=True)[0]
    logging.info(f"joke: {joke}")
    
    return func.HttpResponse(joke)
