#!/usr/bin/env python3

import subprocess
import threading
import time
import psutil

def spawn(args, name):
    if name is not None:
        print("Starting '{}'".format(name))
    cmdl = "cmd /c {}".format(args)
    devnull = subprocess.DEVNULL 
    return (subprocess.Popen(cmdl, stdin=devnull).pid, name)

def kill_safe(process):
    try:
        process.kill()
    except psutil.NoSuchProcess:
        pass

def kill(pid, name=None):
    if pid is None:
        return
    if name is not None:
        print("Killing '{}'".format(name))
    parent = psutil.Process(pid)
    for child in parent.children(recursive=True):
        kill_safe(child)
    kill_safe(parent)

def wait(secs):
    print("Waiting {}s".format(secs))
    time.sleep(5)

def pause():
    print("Press <enter> to restart or <q+enter> to stop...")
    return input()

if __name__ == '__main__':
    while True:
        pids = [
            spawn("nx fable -w", "Fable"),
            spawn("nx webpack --watch", "Webpack"),
            spawn("nx http-server ./out", "Server")
        ]

        wait(10)
        command = pause().strip().lower()

        for pid in pids:
            kill(*pid)

        if command == 'q':
            break

    print("Done.")
