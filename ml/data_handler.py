import logging
from threading import Thread
from queue import Queue


class DataHandler:

    def __init__(self, conn):
        self.logger = logging.getLogger("anha")
        self.rawQueue = Queue()
        self.processedQueue = Queue()
        dataObtainer = DataObtainer(conn, self.rawQueue)
        dataProcessor = DataProcessor(self.rawQueue, self.processedQueue)
        dataObtainer.start()
        dataProcessor.start()


class DataObtainer(Thread):

    def __init__(self, conn, queue):
        Thread.__init__(self)
        self.queue = queue
        self.logger = logging.getLogger("anha")
        self.conn = conn

    def run(self):
        while True:
            data = self.conn.recv(4096)
            if data:
                self.queue.put_nowait(data)




class DataProcessor(Thread):

    def __init__(self, queue, processedQueue):
        Thread.__init__(self)
        self.queue = queue
        self.processedQueue = processedQueue
        self.logger = logging.getLogger("anha")

    def process(self, data):
        self.processedQueue.put(data)

    def run(self):
        while True:
            if not self.queue.empty:
                self.process(self.queue.get())




