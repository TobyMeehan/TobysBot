import ICommand from "./ICommand";
import CommandMessage from "./CommandMessage";

namespace CommandRegistry {
    type Constructor<T> = {
        new (): T;
        readonly prototype: T;
    }

    const implementations: Constructor<ICommand>[] = [];
    export function getImplementations(): Constructor<ICommand>[] {
        return implementations;
    }

    export function register<T extends Constructor<ICommand>>(ctor: T) {
        implementations.push(ctor);
        return ctor;
    }
}

export = CommandRegistry;