#!/usr/bin/env python3

import subprocess
import threading
import time
import psutil

def spawn(command, name):
    if name is not None:
        print("Starting '{}'".format(name))
    cmdl = "cmd /c {}".format(command)
    devnull = subprocess.DEVNULL 
    return (subprocess.Popen(cmdl, stdin=devnull).pid, name)

def safe_find_process(pid):
    try:
        return psutil.Process(pid)
    except psutil.NoSuchProcess:
        pass

def safe_kill_process(process):
    try:
        process.kill()
    except psutil.NoSuchProcess:
        pass

def kill(pid, name=None):
    if pid is None:
        return
    if name is not None:
        print("Killing '{}'".format(name))
    parent = safe_find_process(pid)
    if parent is not None:
        for child in parent.children(recursive=True):
            safe_kill_process(child)
        safe_kill_process(parent)

def watch(command, name):
    stop = threading.Event()
    done = threading.Event()
    pid = None

    def _spawn():
        nonlocal pid
        pid, _ = spawn(command, name)

    def _kill():
        nonlocal pid
        kill(pid, name)
        pid = None
    
    def _test():
        return safe_find_process(pid) is not None

    def _join():
        stop.set()
        done.wait() 

    def _loop():
        try:
            _spawn()
            while True:
                stop.wait(1)
                if stop.is_set():
                    _kill()
                    break
                elif not _test():
                    print("Restarting '{}'".format(name))
                    wait(5)
                    _spawn()
        finally:
            done.set()

    thread = threading.Thread(target=_loop)
    thread.start()
    return _join

def wait(secs):
    print("Waiting {}s".format(secs))
    time.sleep(5)

def pause():
    print("Press <enter> to restart or <q+enter> to stop...")
    return input()

if __name__ == '__main__':
    while True:
        finalizers = [
            watch("nx fable -w", "Fable"),
            watch("nx webpack --watch", "Webpack"),
            watch("nx http-server ./out", "Server")
        ]

        wait(10)
        command = pause().strip().lower()

        for finalizer in finalizers:
            finalizer()

        if command == 'q':
            break

    print("Done.")
