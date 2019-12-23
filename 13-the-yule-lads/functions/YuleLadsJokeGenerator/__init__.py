import azure.functions as func
from textgenrnn import textgenrnn
import tempfile

import urllib.request
import os
import logging

def main(myblob: func.InputStream):
    logging.info(f"Python blob trigger function processed blob \n"
                 f"Name: {myblob.name}\n"
                 f"Blob Size: {myblob.length} bytes")

    tempFilePath = tempfile.gettempdir()
    jokesfile = os.path.join(tempFilePath, "jokes.hdf5")
    urllib.request.urlretrieve(myblob.uri, jokesfile)
    textgen = textgenrnn(jokesfile)
    joke = textgen.generate(return_as_list=True)[0]
    logging.info(f"joke: {joke}")