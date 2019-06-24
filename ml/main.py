import logging
from multiprocessing import freeze_support
import time
from utils.concurrent_file_logger_handler import ConcurrentFileHandler
from connection import ConnectionHandler
from coach import Coach

host = ""
port = 7000


def main():
    logger = logging.getLogger("anha")
    logging.basicConfig(level=logging.DEBUG)

    error_handler = ConcurrentFileHandler("logs/" + time.strftime("%Y%m%d") + "-ERROR.log")
    error_handler.setLevel(logging.ERROR)
    error_handler.setFormatter(logging.Formatter('%(asctime)s - %(filename)s - %(funcName)s - %(message)s'))

    debug_handler = ConcurrentFileHandler("logs/" + time.strftime("%Y%m%d") + "-DEBUG.log")
    debug_handler.setLevel(logging.DEBUG)
    debug_handler.setFormatter(logging.Formatter('%(asctime)s - %(levelname)s - %(filename)s - %(funcName)s - %(message)s'))

    info_handler = ConcurrentFileHandler("logs/" + time.strftime("%Y%m%d") + "-INFO.log")
    info_handler.setLevel(logging.INFO)
    info_handler.setFormatter(logging.Formatter('%(message)s'))

    logger.addHandler(error_handler)
    logger.addHandler(debug_handler)
    logger.addHandler(info_handler)

    teacher = Coach("../effortResult.csv", "../effortComputed.csv")
    teacher.assess()


if __name__ == '__main__':
    freeze_support()
    main()
