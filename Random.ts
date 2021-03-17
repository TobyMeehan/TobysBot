class Random {
    static select<T>(items: T[]): T {
        return items[Math.floor(Math.random() * (items.length - 1))];
    }

    static next(min: number, max: number) {
        return (Math.random() * (max - min)) + min;
    }
}

export = Random;