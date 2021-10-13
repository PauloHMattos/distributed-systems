import subprocess

def build():
    result = subprocess.run(["dotnet", "build", "-c", "Release"], stdout=subprocess.PIPE)
    return result.stdout

def run_program(r, k):
    return subprocess.Popen(["bin/Release/net5.0/Process.exe", str(k), str(r)], stdout=subprocess.PIPE, stdin=subprocess.PIPE, stderr=subprocess.PIPE)

def run():
    n = 128
    r = 3
    k = 0
    process = []
    build()
    print("r = {} - k = {}".format(r, k))
    for nElements in range(n):
        print("n = {}".format(nElements))
        process.append(run_program(r, k))

    input("(press enter)")
    for p in process:
        p.terminate()
    

if __name__ == "__main__":
    run()