﻿import React, {Component, FormEvent} from 'react';
import {
    Button,
    ButtonProps,
    Card,
    CardBody,
    CardHeader, CardText, CardTitle,
    Collapse,
    Container,
    Input,
    Nav,
    NavItem,
    NavLink
} from "reactstrap";
import {Command} from "../models/Command";
import {Plugin} from "../models/Plugin";
import {Helmet} from "react-helmet";

export class Commands extends Component<{}, State> {
    static displayName = Commands.name;

    constructor(props: any) {
        super(props);
        this.state = {
            plugins: [],
            commands: [],
            commandFilter: "",
            pluginFilter: [],
            loading: true
        }
    }

    componentDidMount() {
        this.loadCommands();
    }

    render() {
        return (
            <>
                <Helmet>
                    <title>Commands</title>
                </Helmet>
                <Container className='pt-3'>
                    <div className='row'>
                        <div className='col-12 text-center'>
                            <h2>Command List</h2>
                            <p>A list of all of Toby's Bot's commands.</p>
                        </div>
                    </div>
                    <div className='row mb-3'>
                        <div className='col-md-2'></div>
                        <div className='col-md-8'>
                            <Input type='search' name='search' id='search' placeholder='Search'
                                   bsSize='lg' className='bg-dark border-dark text-light'
                                   onChange={e => this.filterCommands(e.target.value)}
                            />
                        </div>
                        <div className='col-md-2'></div>
                    </div>
                    <div className='row'>
                        <div className='col-md-4'>
                            <Card className='bg-600'>
                                <CardBody>
                                    {
                                        this.state.plugins.map(x =>
                                            <Button outline={!x.selected} color='primary' className='w-100 mb-2'
                                                    onClick={() => this.filterPlugins(x.id)}>
                                                {x.name}
                                            </Button>
                                        )
                                    }
                                </CardBody>
                            </Card>
                        </div>
                        <div className='col-md-8'>
                            {
                                this.getCommands().map(command =>
                                    <>
                                        <Card className='bg-600 mb-3'>
                                            <CardBody>
                                                <CardTitle>
                                                    <span className='fw-bold'>{`/${command.name}`}</span>
                                                    <span className='text-muted'>{
                                                        command.parameters.map(x =>
                                                            <>
                                                                {' '}[{x}]
                                                            </>)
                                                    }</span>
                                                </CardTitle>
                                                <CardText>{command.description}</CardText>
                                            </CardBody>
                                        </Card>
                                    </>)
                            }
                        </div>
                    </div>
                </Container>
            </>
        );
    }

    getCommands(): StateCommand[] {
        return this.state.commands
            .filter(x =>
                x.name.includes(this.state.commandFilter) &&
                this.state.plugins.find(p => p.id === x.pluginId)?.selected);
    }

    filterCommands(search: string) {
        this.setState(x => ({
            commandFilter: search
        }));
    }

    filterPlugins(pluginId: string) {
        this.setState(state => ({
            plugins: state.plugins.map(plugin => {
                if (plugin.id === pluginId) {
                    return {...plugin, selected: !plugin.selected}
                }

                return plugin;
            })
        }));
    }

    openCommand(name: string) {
        this.setState(state => ({
            commands: state.commands.map(command => {
                if (command.name === name) {
                    return {...command, selected: !command.open}
                }

                return command;
            })
        }));
    }

    async loadCommands() {
        let response = await fetch('data/commands/commands');
        const commands: Command[] = await response.json();

        this.setState(({
            commands: commands.map(x => ({
                pluginId: x.pluginId,
                name: x.name,
                parameters: x.parameters,
                description: x.description,
                open: false
            }))
        }));

        response = await fetch('data/commands/plugins');
        const plugins: Plugin[] = await response.json();

        this.setState(({
            plugins: plugins.map(x => ({
                id: x.id,
                name: x.name,
                description: x.description,
                selected: true
            })),
            loading: false
        }));
    }
}

interface StateCommand extends Command {
    open: boolean;
}

interface StatePlugin extends Plugin {
    selected: boolean;
}

interface State {
    commands: StateCommand[];
    plugins: StatePlugin[];
    commandFilter: string;
    pluginFilter: string[];
    loading: boolean;
}