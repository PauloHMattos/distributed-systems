import matplotlib.pyplot as plt
import csv

def read_csv(path: str) -> tuple:
    results = {}
    xticks = set([])

    with open(path, 'r') as csv_file:
        dataset = csv.reader(csv_file, delimiter=',')
        next(dataset)
        for line in dataset:
            nElements, nThreads, time = line
            if nElements not in results:
                graph = {
                    "x": [], "y": []
                }
                results[nElements] = graph

            x = int(nThreads.strip())
            xticks.add(x)
            results[nElements]["x"].append(x)
            results[nElements]["y"].append(float(time.strip()))

    return results, list(xticks)



if __name__ == '__main__':
    results, xticks = read_csv("data.csv")

    fig = plt.figure(figsize=(12.8, 9.6))
    ax = fig.add_subplot()
    ax.set(xlabel='Threads', ylabel='Tempo (ms)')

    ax.yaxis.grid(True, which='both')
    ax.xaxis.grid(True, which='minor')
    ax.set_xticks(xticks, minor=True)
    
    ax.set_title('Numero de Threads vs Tempo (ms)')
    for key, result in results.items():
        ax.plot(result["x"], result["y"], label = "N = {}".format(key))

    ax.legend(loc='best', fontsize='x-large')
    plt.savefig('foo.png')