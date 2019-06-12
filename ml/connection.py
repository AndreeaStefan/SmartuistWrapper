import logging
import socket
import sys


class ConnectionHandler:

    def __init__(self):
        self.logger = logging.getLogger("anha")

    def start(self, host, port):
        s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.logger.debug("Socket created")
        try:
            s.bind((host, port))
        except socket.error as msg:
            self.logger.error("binding socket failed to {} on port {}. MSG:{}".format(host, port, msg))
            sys.exit()

        self.logger.debug('Socket bind complete')
        s.listen(5)
        conn, addr = s.accept()
        self.logger.debug("Connected to {}:{}".format(addr[0], str(addr[1])))
        return conn






