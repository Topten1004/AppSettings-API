export class DeviceDynamicProperty
{
    propertyId?: number;
    displayText: string;
    name: string;
    value: string;
    placeHolder: string;
    type:string;
    isReadOnly:boolean
    constructor() {
       this.displayText='';
        this.name = '';
        this.value='';
        this.placeHolder='';
        this.type='text';
        this.isReadOnly=false;
    }
}