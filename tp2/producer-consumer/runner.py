import matplotlib.pyplot as plt
import numpy as np
import subprocess

buffer_sizes = ["1", "2", "4", "8", "16", "32"]
threads_input = ["1", "2", "4", "8", "16"]

results = {}

for buffer_size in buffer_sizes:
    x = []
    y = []

    for ti in threads_input:
        counter = 0
        for i in range(10):
            process = subprocess.Popen("dotnet run -- " + buffer_size + " 1 " + ti, stdout=subprocess.PIPE, shell=True)
            (output, err) = process.communicate()
            counter += int(output)
        print ("With buffer size " + buffer_size + ", 1 producer and " + ti + " consumers, it took: " + str(counter/10))

        x.append(int(ti))
        y.append(counter/10)

    graph = {
        "x": x, "y": y
    }
    results[buffer_size] = graph

for key, result in results.items():
    plt.plot(result["x"], result["y"], label = key)

plt.title("Numero de Threads vs Tempo (ms)")
plt.xlabel("Threads consumidoras")
plt.ylabel("Tempo (ms)")
plt.legend()
plt.savefig("1_producer_n_consumers.png")
plt.clf()

for buffer_size in buffer_sizes:
    x = []
    y = []

    for ti in threads_input:
        counter = 0
        for i in range(10):
            process = subprocess.Popen("dotnet run -- " + buffer_size + " " + ti + " 1", stdout=subprocess.PIPE, shell=True)
            (output, err) = process.communicate()
            counter += int(output)
        print ("With buffer size " + buffer_size + ", " + ti + " producers and 1 consumer, it took: " + str(counter/10))

        x.append(int(ti))
        y.append(counter/10)

    graph = {
        "x": x, "y": y
    }
    results[buffer_size] = graph

for key, result in results.items():
    plt.plot(result["x"], result["y"], label = key)

plt.title("Numero de Threads vs Tempo (ms)")
plt.xlabel("Threads produtoras")
plt.ylabel("Tempo (ms)")
plt.legend()
plt.savefig("n_producers_1_consumer.png")