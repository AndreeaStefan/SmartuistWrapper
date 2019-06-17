import logging
from data_handler import DataHandler


class Teacher:

    def __init__(self, fileName):
        self.logger = logging.getLogger("anha")
        self.dataHandler = DataHandler(fileName)

    def teach(self):
        while True:
            self.logger.info(self.dataHandler.getData())


