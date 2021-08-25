import matplotlib.pyplot as plt
import numpy as np
import subprocess

buffer_sizes = ["1"]#, "2", "4", "8", "16", "32"]
threads_input = [
    ["1", "16"],
    ["1", "8"],
    ["1", "4"],
    ["1", "2"],
    ["1", "1"],
    ["2", "1"],
    ["4", "1"],
    ["8", "1"],
    ["16", "1"]
]

results = {}
output_path = "data.csv"

with open(output_path, "w", newline='') as f:
    f.write("Buffer size,# producer threads,# consumer threads,Time(ms)\n")
    for buffer_size in buffer_sizes:
        x = []
        y = []

        for ti in threads_input:
            counter = 0
            for i in range(10):
                process = subprocess.Popen("dotnet run -c Release -- " + ti[0] + " " + ti[1] + " " + buffer_size, stdout=subprocess.PIPE, shell=True)
                (output, err) = process.communicate()
                counter += int(output)
            print ("With buffer size " + buffer_size + ", " + ti[0] + " producers and " + ti[1] + " consumers, it took: " + str(counter/10))

            f.write("{}, {}, {}, {}\n".format(buffer_size, ti[0], ti[1], counter/10))
            f.flush
            x.append(int(ti[0])/int(ti[1]))
            y.append(counter/10)

        graph = {
            "x": x, "y": y
        }
        results[buffer_size] = graph

for key, result in results.items():
    plt.plot(result["x"], result["y"], label = key)

plt.title("Numero de Threads vs Tempo (ms)")
plt.xlabel("Np/Nc")
plt.xscale("log", basex=2)
plt.ylabel("Tempo (ms)")
plt.legend()
plt.savefig("producer_consumer.png")
plt.clf()
