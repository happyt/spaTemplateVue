import Vue from 'vue';
import { Component } from 'vue-property-decorator';
 
interface SheetItem {
    description: string;
    done: boolean;
    id: string;
}
 
@Component
export default class SheetComponent extends Vue {
    sheets: SheetItem[];
    newItemDescription: string;
 
    data() {
        return {
            sheets : [],
            newItemDescription: null
        };
    }
    mounted() {
        fetch('/api/sheet')
            .then(response => response.json() as Promise<SheetItem[]>)
            .then(data => {
                this.sheets = data;
            });
    }
    completeItem(item: SheetItem){
        fetch(`/api/sheet/${item.id}`, {
            method: 'delete',
            headers: new Headers({
            'Accept': 'application/json',
            'Content-Type': 'application/json'
            })
        })
        .then(() => {
            this.sheets = this.sheets.filter((t) => t.id !== item.id);
        });
    }
    addItem(event: Event){
        if(event) event.preventDefault();
         
        fetch('/api/sheet', {
            method: 'post',
            body: JSON.stringify(<SheetItem>{description: this.newItemDescription}),
            headers: new Headers({
            'Accept': 'application/json',
            'Content-Type': 'application/json'
            })
        })
        .then(response => response.json() as Promise<SheetItem>)
        .then((newItem) => {
            this.sheets.push(newItem);
            this.newItemDescription = "";
        });
    }
}