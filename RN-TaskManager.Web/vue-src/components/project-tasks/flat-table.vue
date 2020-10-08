<template>
    <div >

        <v-data-table class="table-tasks"
                      :headers="headers"
                      :items="items"
                      :loading="loading"
                      dense="true"
                      hide-default-footer="true"
                      @click:row="onRowClick"
                      :custom-sort="customSort"
                      :item-class="rowClass"
                      items-per-page="500">

            <template v-slot:item.TaskTypeName="{ item }">
                <span>
                    <v-tooltip top>
                        <template v-slot:activator="{ on, attrs }">
                            <b v-bind="attrs"
                               v-on="on">
                                {{$abbreviation(item.TaskTypeName)}}
                            </b>
                        </template>
                        <span>{{item.TaskTypeName}}</span>
                    </v-tooltip>
                </span>
            </template>

            <template v-slot:item.StartPlan="{ item }">
                <span>{{$localeDateFormat(item.StartPlan)}}</span>
            </template>

            <template v-slot:item.EndPlan="{ item }">
                <span>{{$localeDateFormat(item.EndPlan)}}</span>
            </template>

            <template v-slot:item.StartFact="{ item }">
                <span>{{$localeDateFormat(item.StartFact)}}</span>
            </template>

            <template v-slot:item.EndFact="{ item }">
                <span>{{$localeDateFormat(item.EndFact)}}</span>
            </template>


            <template v-slot:item.TaskStatusName="{ item }">
                <div>
                    <v-tooltip top>
                        <template v-slot:activator="{ on, attrs }">
                            <v-icon small
                                    :style="{color: item.TaskStatus.StatusColor}"
                                    v-bind="attrs"
                                    v-on="on">
                                mdi-brightness-1
                            </v-icon>
                        </template>
                        <span>{{item.TaskStatusName}}</span>
                    </v-tooltip>
                </div>
            </template>

            <template slot="no-data">
                <span>Записей нет</span>
            </template>

        </v-data-table>

    </div>
</template>

<script>

    import { localeDateFormat } from '../../helpers/date-helpers'
    import { abbreviation } from '../../helpers/string-helpers'

    Vue.prototype.$localeDateFormat = localeDateFormat;
    Vue.prototype.$abbreviation = abbreviation;

    export default {
        props: {
            items: Array,
            excludeHeaders: Array,
            loading: Boolean,
        },
        data: function () {
            return {
                headers: [
                    { text: '', value: 'TaskStatusName', width: '30px', sortable: false },
                    { text: '', value: 'TaskTypeName', width: '40px' },

                    { text: 'Проект', value: 'ProjectName', filterable: true },
                    { text: 'Описание работы', value: 'Details' },

                    { text: 'Начало план', value: 'StartPlan', width: '145px' },
                    { text: 'Окончание план', value: 'EndPlan', width: '145px' },
                    { text: 'Начало факт', value: 'StartFact', width: '145px' },
                    { text: 'Окончание факт', value: 'EndFact', width: '145px' },

                    { text: 'Исполнитель', value: 'Performers' },

                    { text: 'Группа', value: 'GroupName', width: '145px' },
                ],
            }
        },
        created: function () {
            if (this.excludeHeaders !== undefined) {

                let headerValues = this.headers.map(e => e.value);

                for (let headerName of this.excludeHeaders) {
                    let i = headerValues.indexOf(headerName);
                    this.headers.splice(i, 1);
                }
            }
        },
        methods: {

            customSort(items, index, isDesc) {

                if (index[0] === undefined)
                    return items;

                items.sort((a, b) => {
                    if (["StartPlan", "EndPlan", "StartFact", "EndFact"].indexOf(index[0]) > -1) {
                        if (!isDesc[0]) {
                            return ('' + a[index[0]]).localeCompare(b[index[0]]);
                        } else {
                            return ('' + b[index[0]]).localeCompare(a[index[0]]);
                        }
                    } else {
                        if (!isDesc[0]) {
                            return a[index[0]] < b[index[0]] ? -1 : 1;
                        } else {
                            return b[index[0]] < a[index[0]] ? -1 : 1;
                        }
                    }
                });

                return items;
            },
            rowClass(e) {

                if (e.EndPlan === null || e.EndFact !== null)
                    return;

                let dNow = Date.now();
                let dEnd = new Date(e.EndPlan).getTime();

                if (dEnd < dNow)
                    return 'red lighten-4';

            },
            onRowClick(e) {
                this.$emit('select', e);
            },
        },
    };
</script>

<style>

    .col {
        padding: .15rem !important;
    }

    .table-tasks .v-data-table-header th {
        padding: 0 10px !important;
    }

    .table-tasks tbody td {
        cursor: pointer;
    }

    .table-tasks tr.red.lighten-4:hover {
        background-color: rgba(255, 205, 210, 0.70) !important;
    }

</style>