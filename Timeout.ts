class Timeout {
    static sleep(ms: number) {
        return new Promise(x => setTimeout(x, ms));
    }
}

export = Timeout;