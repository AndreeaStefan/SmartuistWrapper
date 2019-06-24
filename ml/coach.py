import logging
from random import randint, random
from data_handler import DataHandler
from scipy.spatial import distance
import time
import numpy as np
from datetime import datetime

first = 6
last = 24
indices = [6,9,12,15,18,21,24]
posMain = 6
posHandL = 9
posArmL = 12
posHandR = 15
posArmR = 18
posFootL = 21
posFootR = 24
timeI = 27
rep = 2


def toStr(whatev, deli):
    return deli.join([str(i) for i in whatev])


class Coach:

    def __init__(self, fileName, resultFileName):
        self.logger = logging.getLogger("anha")
        self.dataHandler = DataHandler(fileName)
        self.resultFile = open(resultFileName, "a")
        self.current = []
        self.forEffort = []
        self.distances = []
        self.last = []

    def assess(self):
        lastRead = time.time()
        while True:
            data = self.dataHandler.getData()
            if data:
                lastRead = time.time()
                self.logger.debug("Obtained data: " + toStr(data, ", "))
                if not self.current:
                    self.current.append(data)
                if self.current[0][rep] == data[rep]:
                    self.current.append(data)
                else:
                    self.logger.debug(
                        "Computing effort for Lesson,Repetition: " + str(self.current[0][rep - 1]) + "," + str(self.current[0][rep]))
                    self.forEffort = [i for i in self.current]
                    self.current = [data]
                    self.computeEffort()
            elif time.time() - lastRead > 15:
                if self.current:
                    self.logger.debug(
                        "[INACTIVE] Computing effort for Lesson,Repetition: " + str(self.current[0][rep - 1]) + "," + str(self.current[0][rep]))
                    self.forEffort = [i for i in self.current]
                    self.current = []
                    self.computeEffort()
            else:
                time.sleep(0.01)

    def computeEffort(self):
        data = np.mat(self.forEffort)
        positions = [data[:, range(i, i + 3, 1)] for i in indices]
        positions = [np.vectorize(float)(m) for m in positions]
        time = data[:, [timeI]].tolist()
        for i in range(positions[0].shape[0]):
            if i == 0:
                self.distances = [0] * 7
            else:
                for j in range(7):
                    self.distances[j] += distance.euclidean(positions[j][i], positions[j][i-1])
        start = datetime.strptime(time[0][0], "%Y-%m-%d %H:%M:%S.%f")
        end = datetime.strptime(time[-1][0], "%Y-%m-%d %H:%M:%S.%f")
        # insert repetition
        self.distances.insert(0, self.forEffort[1][2])
        # insert lesson
        self.distances.insert(0, self.forEffort[1][1])
        self.resultFile.write(toStr(self.distances, ",") + "\n")
        self.resultFile.flush()







