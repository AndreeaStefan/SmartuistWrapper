import logging
from data_handler import DataHandler


class Teacher:

    def __init__(self, conn):
        self.logger = logging.getLogger("anha")
        self.dataHandler = DataHandler(conn)

    def teach(self):
        while True:
            self.logger.debug(self.dataHandler.processedQueue.get())


