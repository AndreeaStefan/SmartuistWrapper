import logging
from random import randint, random
from data_handler import DataHandler
from assessor import Assessor
import time
import operator

populus = 5
bits = 9

def bitfield(n):
    inBits = [1 if digit == '1' else 0 for digit in bin(n)[2:]]
    while len(inBits) < bits:
        inBits.insert(0, 0)
    return inBits


def bitArrayToInt(bitlist):
    out = 0
    for bit in bitlist:
        out = (out << 1) | bit
    return out


class Teacher:

    def __init__(self, fileName, resultFileName, mutationChance):
        self.logger = logging.getLogger("anha")
        self.dataHandler = DataHandler(fileName)
        self.resultFile = open(resultFileName, "a")
        self.numbers = []
        self.mutationChance  = mutationChance

    def getInitialPopulation(self, num, min, max):
        return [bitfield(randint(min, max)) for i in range(num)]

    def savePopulation(self, pop):
        self.numbers = [bitArrayToInt(bitArray) for bitArray in pop]
        for number in self.numbers:
            self.resultFile.write("%d\n" % number)

    def evolve(self, results):
        self.logger.debug("About to evolve based on results: " + ", ".join(results))
        oldPop = [j for i, j in results]
        selNum = int(len(results)/2)
        first = int(bits/2)
        second = bits - first
        left = populus - selNum
        selected = oldPop[:selNum]

        for i in range(left):
            child = oldPop[i][:first] + oldPop[i+1][:second]
            self.logger.debug("Parent#1: " + ", ".join(oldPop[i]) + "\nParent#2: " + ", ".join(oldPop[i+1]) + "\nChild: " + ", ".join(child))
            selected.append(child)

        return selected

    def mutate(self, pop):
        for single in pop:
            if random() < self.mutationChance:
                place_to_modify = randint(0, bits)
                single[place_to_modify] = 1 if single[place_to_modify] == 0 else 0
        return pop


    def teach(self):
        population = self.getInitialPopulation(populus, 100, 300)
        self.savePopulation(population)
        lesson = 0
        while True:
            endedTrials = 0
            i = 0
            records = [[] for i in range(populus)]
            while endedTrials < populus:
                data = self.dataHandler.getData()
                if data:
                    if not records[i]:
                        records[i].append(data)
                    elif records[i][0][1] != data[1]:
                        i = i + 1
                        self.logger.debug("Collected info from target: %d" % endedTrials)
                        endedTrials = endedTrials + 1
                        records[i].append(data)
                    else:
                        records[i].append(data)
                else:
                    time.sleep(0.01)

            results = list(zip(Assessor.assess(records), population))
            results.sort(key=operator.itemgetter(0))

            population = self.evolve(results)
            self.logger.debug("After evolution: " + ", ".join(population))
            population = self.mutate(population)
            self.logger.debug("After mutation: " + ", ".join(population))
            self.savePopulation(population)
            self.logger.debug("Finished lesson %d" % lesson)
            lesson = lesson + 1


