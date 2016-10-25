import {customElement} from 'aurelia-framework'

@customElement('binding-demo')
export class BindingDemo
{
    firstName: string = "firstName from ViewModel";
    lastName: string = "lastName from ViewModel"
    color:string = "white";

    get cssClass(): string {
    return `background: ${this.color};`;
  }


    select(yourChoice: string)
    {
        alert(yourChoice);
    }
}