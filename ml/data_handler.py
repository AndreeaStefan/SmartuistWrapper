import logging
from threading import Thread
from queue import Queue
import time
import numpy as np


def obtainData(file, queue):
    file.seek(0, 2)
    while True:
        line = file.readline()
        if not line:
            time.sleep(0.1)
            continue
        queue.put(line)



class DataHandler:

    def __init__(self, fileName):
        self.logger = logging.getLogger("anha")
        self.queue = Queue()
        file = open(fileName, "r")
        thread = Thread(target=obtainData, args=[file, self.queue])
        thread.start()

    def getData(self):
        return self.queue.get()


