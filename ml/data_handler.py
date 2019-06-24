import logging
from threading import Thread
from queue import Queue, Empty
import time


def obtainData(file, queue):
    # file.seek(0, 2)
    while True:
        last_pos = file.tell()
        line = file.readline()
        if not line:
            time.sleep(0.1)
            continue
        elif not line.endswith('\n'):
            file.seek(last_pos)
        else:
            splitLine = line[:-1].split(",")
            if len(splitLine) == 28:
                queue.put(splitLine)



class DataHandler:

    def __init__(self, fileName):
        self.logger = logging.getLogger("anha")
        self.queue = Queue()
        file = open(fileName, "r")
        thread = Thread(target=obtainData, args=[file, self.queue])
        thread.start()

    def getData(self):
        try:
            return self.queue.get(block=False)
        except Empty:
            return []


