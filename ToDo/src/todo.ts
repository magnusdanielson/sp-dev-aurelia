export class Todo
{
    description: string;
    done: boolean;
    constructor(description)
    {
        this.description = description;
        this.done = false;
    }
}