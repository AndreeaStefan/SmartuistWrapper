from random import randint


class Assessor:

    @staticmethod
    def assess(records):
        # do some evaluation on the records
        size = len(records)
        return [randint(0, size) for i in range(size)]


