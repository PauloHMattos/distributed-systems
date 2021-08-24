import subprocess
import matplotlib.pyplot as plt

def run_program(threads, elements):
    result = subprocess.run(["dotnet", "run", "-c", "Release", str(threads), str(elements)], stdout=subprocess.PIPE)
    return float(result.stdout.decode('utf-8').replace(',','.'))

def run():
    threadsCount = [1, 2, 4, 8, 16, 32, 64, 128, 256]
    elementsCount = [10**7, 10**8, 10**9]

    output_path = 'data.csv'
    with open(output_path, 'w', newline='') as f:
        f.write("# N Elements; N Threads; Time(ms)\n")
        for nElements in elementsCount:
            for nThreads in threadsCount:
                print("Elements {} - Threads {}".format(nElements, nThreads))
                time = run_program(nThreads, nElements)
                f.write("{}, {}, {}\n".format(nElements, nThreads, time))
                f.flush()

    
if __name__ == "__main__":
    run()