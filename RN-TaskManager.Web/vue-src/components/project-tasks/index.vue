<template>
    <div class="component-container">

        <v-toolbar flat color="white">
            <v-toolbar-title>{{title}}</v-toolbar-title>

            <v-divider class="mx-4"
                       inset
                       vertical></v-divider>

            <v-btn @click="onChangeMyTask" class="mr-2">
                <v-icon small
                        :color="filters.myTask ? 'green' : 'grey'"
                        class="mr-2">
                    mdi-brightness-1
                </v-icon>
                Мои задачи
            </v-btn>

            <v-btn @click="onChangeFilterWeek" class="mr-2">
                <v-icon small
                        :color="filters.week ? 'green' : 'grey'"
                        class="mr-2">
                    mdi-brightness-1
                </v-icon>
                На неделю
            </v-btn>

            <v-spacer></v-spacer>

            <v-dialog v-model="dialog" max-width="800px">
                <template v-slot:activator="{ on, attrs }">
                    <v-btn color="primary"
                           dark
                           class=""
                           v-bind="attrs"
                           v-on="on">Добавить запись</v-btn>
                </template>
                <v-card>
                    <v-card-title>
                        <v-tooltip top>
                            <template v-slot:activator="{ on, attrs }">
                                <v-icon color="blue" v-bind="attrs" v-on="on">mdi-details</v-icon>
                            </template>
                            <span>
                                {{getDetailsString('Создано ', editedItem.LoginCreated, editedItem.DateCreated)}}<br />
                                {{getDetailsString('Изменено ', editedItem.LoginEdited, editedItem.DateEdited)}}<br />
                                {{getDetailsString('Удалено ', editedItem.LoginDeleted, editedItem.DateDeleted)}}
                            </span>
                        </v-tooltip>

                        <span class="headline ml-2">Карточка задачи</span>
                    </v-card-title>

                    <v-card-text>
                        <v-container>
                            <v-row>
                                <v-col cols="12">
                                    <v-textarea label="Описание работы" v-model="editedItem.Details" rows="2" dense></v-textarea>
                                </v-col>
                            </v-row>

                            <v-row>
                                <v-col cols="5">
                                    <v-select :items="projects"
                                              item-text="ProjectName"
                                              item-value="ProjectId"
                                              v-model="editedItem.ProjectId"
                                              v-on:change="loadTaskTypes"
                                              :rules="[e => e > 0]"
                                              dense
                                              label="Проект"></v-select>
                                </v-col>
                                <v-col>
                                    <v-select :items="taskTypes"
                                              item-text="ProjectTaskTypeName"
                                              item-value="ProjectTaskTypeId"
                                              v-model="editedItem.ProjectTaskTypeId"
                                              :rules="[e => e > 0]"
                                              :loading="loadings.taskTypes"
                                              :disabled="loadings.taskTypes"
                                              dense
                                              label="Тип задачи"></v-select>
                                </v-col>
                                <v-col cols="3">
                                    <v-select :items="statuses"
                                              item-text="StatusName"
                                              item-value="ProjectTaskStatusId"
                                              v-model="editedItem.ProjectTaskStatusId"
                                              :rules="[e => e > 0]"
                                              dense
                                              label="Статус"></v-select>
                                </v-col>
                            </v-row>

                            <v-row>
                                <v-col cols="5">
                                    <v-select :items="groups"
                                              item-text="GroupName"
                                              item-value="GroupId"
                                              v-model="editedItem.GroupId"
                                              :rules="[e => e > 0]"
                                              dense
                                              label="Группа"></v-select>
                                </v-col>

                                <v-col>
                                    <v-select v-model="editedItem.Users"
                                              :items="users.filter(e => e.GroupId === editedItem.GroupId)"
                                              item-text="ShortName"
                                              item-value="UserId"
                                              attach
                                              dense
                                              single-line
                                              label="Исполнители"
                                              multiple></v-select>
                                </v-col>
                            </v-row>

                            <v-row>
                                <v-col>
                                    <datePicker v-model="editedItem.StartPlan" label="Плановое начало"></datePicker>
                                </v-col>
                                <v-col>
                                    <datePicker v-model="editedItem.EndPlan" label="Плановое окончание"></datePicker>
                                </v-col>
                                <v-col cols="1">
                                </v-col>
                                <v-col cols="3">
                                    <v-text-field v-model="editedItem.Priority"
                                                  label="Приоритет"
                                                  type="number"
                                                  dense
                                                  :rules="[e => e >= 0 ]">
                                    </v-text-field>
                                </v-col>
                            </v-row>

                            <v-row>
                                <v-col>
                                    <datePicker v-model="editedItem.StartFact" label="Фактическое начало"></datePicker>
                                </v-col>
                                <v-col>
                                    <datePicker v-model="editedItem.EndFact" label="Фактическое окончание"></datePicker>
                                </v-col>
                                <v-col cols="1">
                                </v-col>
                                <v-col cols="3">
                                    <v-text-field v-model="editedItem.DurationHours"
                                                  label="Трудозатраты чел/час"
                                                  type="number"
                                                  dense
                                                  :rules="[e => e >= 0 ]">
                                    </v-text-field>
                                </v-col>
                            </v-row>

                            <v-row>
                                <v-col>
                                    <v-textarea label="Примечание" v-model="editedItem.Note" rows="2" dense></v-textarea>
                                </v-col>
                            </v-row>

                        </v-container>
                    </v-card-text>

                    <v-card-actions>
                        <v-btn color="red darken-1" text @click="deleteItem">Удалить</v-btn>

                        <v-spacer></v-spacer>
                        <v-btn color="blue darken-1" text @click="close">Отмена</v-btn>
                        <v-btn color="blue darken-1"
                               text @click="save"
                               :loading="loadings.save"
                               :disabled="loadings.save">Сохранить</v-btn>
                    </v-card-actions>
                </v-card>
            </v-dialog>
        </v-toolbar>

        <v-divider></v-divider>

        <div class="d-flex mt-2 toolbar-filters">

            <v-text-field v-model="search"
                          class=""
                          style="max-width: 15rem;"
                          label="Поиск"
                          outlined
                          dense
                          clearable></v-text-field>

            <v-select v-model="filters.status"
                      class="ml-2"
                      style="max-width: 12rem;"
                      :items="statuses"
                      item-text="StatusName"
                      item-value="ProjectTaskStatusId"
                      label="Статус"
                      outlined
                      dense
                      multiple>
                <template v-slot:selection="{ item, index }">
                    <span class="grey--text caption" v-if="index === 0">Выбрано {{ filters.status.length }}</span>
                </template>
            </v-select>

            <v-select v-model="filters.projects"
                      class="ml-2"
                      style="max-width: 15rem;"
                      :items="projects"
                      item-text="ProjectName"
                      item-value="ProjectId"
                      label="Проект"
                      outlined
                      dense
                      multiple>
                <template v-slot:selection="{ item, index }">
                    <span class="grey--text caption" v-if="index === 0">Выбрано {{ filters.projects.length }}</span>
                </template>
            </v-select>

            <v-select v-model="filters.groups"
                      class="ml-2"
                      style="max-width: 10rem;"
                      :items="groups"
                      item-text="GroupName"
                      item-value="GroupId"
                      label="Группа"
                      outlined
                      dense
                      multiple>
                <template v-slot:selection="{ item, index }">
                    <span class="grey--text caption" v-if="index === 0">Выбрано {{ filters.groups.length }}</span>
                </template>
            </v-select>

            <v-select v-model="filters.performers"
                      class="ml-2"
                      style="max-width: 15rem;"
                      :items="users"
                      item-text="ShortName"
                      item-value="UserId"
                      label="Исполнитель"
                      outlined
                      dense
                      multiple>
                <template v-slot:selection="{ item, index }">
                    <span class="grey--text caption" v-if="index === 0">Выбрано {{ filters.performers.length }}</span>
                </template>
            </v-select>


        </div>

        <v-divider></v-divider>

        <v-data-table class="table-tasks"
                      :headers="headers"
                      :items="tasks"
                      :loading="loadings.users || loadings.firstLoad"
                      dense="true"
                      hide-default-footer="true"
                      @click:row="onRowClick"
                      :custom-sort="customSort"
                      items-per-page="500">

            <template v-slot:item.TaskTypeName="{ item }">
                <span>
                    <v-tooltip top>
                        <template v-slot:activator="{ on, attrs }">
                            <b v-bind="attrs"
                               v-on="on">
                                {{abbreviation(item.TaskTypeName)}}
                            </b>
                        </template>
                        <span>{{item.TaskTypeName}}</span>
                    </v-tooltip>
                </span>
            </template>

            <template v-slot:item.StartPlan="{ item }">
                <span>{{localeFormat(item.StartPlan)}}</span>
            </template>

            <template v-slot:item.EndPlan="{ item }">
                <span>{{localeFormat(item.EndPlan)}}</span>
            </template>

            <template v-slot:item.StartFact="{ item }">
                <span>{{localeFormat(item.StartFact)}}</span>
            </template>

            <template v-slot:item.EndFact="{ item }">
                <span>{{localeFormat(item.EndFact)}}</span>
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

        <v-snackbar v-model="snackbar">
            {{ errorMessage }}

            <template v-slot:action="{ attrs }">
                <v-btn color="error"
                       text
                       v-bind="attrs"
                       @click="closeSnackbar">
                    Закрыть
                </v-btn>
            </template>
        </v-snackbar>
    </div>
</template>

<script>
    import datePicker from './date-picker.vue'

    export default {
        data: () => ({
            title: 'Задачи',
            dialog: false,
            snackbar: false,
            errorMessage: '',
            items: [],
            projects: [],
            groups: [],
            statuses: [],
            taskTypes: [],
            users: [],
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
                //{ text: '', value: 'actions', sortable: false, width: '100px' },
            ],
            editedIndex: -1,
            editedItem: {
                ProjectTaskId: 0,
                ProjectId: 0,
                ProjectTaskStatusId: 0,
                ProjectTaskTypeId: 0,
                GroupId: 0,
                Details: '',
                Note: '',
                StartPlan: new Date().toISOString().substr(0, 10),
                EndPlan: '',
                StartFact: '',
                EndFact: '',
                DurationHours: 0,
                Priority: 0,
                Users: [],
            },
            defaultItem: {
                ProjectTaskId: 0,
                ProjectId: 0,
                ProjectTaskStatusId: 0,
                ProjectTaskTypeId: 0,
                GroupId: 0,
                Details: '',
                Note: '',
                StartPlan: new Date(Date.now()).toISOString(),
                EndPlan: '',
                StartFact: '',
                EndFact: '',
                DurationHours: 0,
                Priority: 0,
                Users: [],
            },
            loadings: {
                firstLoad: false,
                items: false,
                performers: false,
                taskTypes: false,
                save: false,
            },
            filters: {
                status: [],
                performers: [],
                projects: [],
                groups: [],
                myTask: true,
                week: false
            },
            search: '',
            api: '/api/projectTasks'
        }),
        created() {
            this.loadItems();
        },
        watch: {
            dialog(val) {
                val || this.close()
            },
        },
        computed: {
            tasks() {

                let items = this.items;

                // фильтр за неделю....
                if (this.filters.week) {

                    let currentDate = new Date;
                    var first = currentDate.getDate() - currentDate.getDay() + 1;
                    var last = first + 6;

                    var firstday = new Date(currentDate.setDate(first));
                    var lastday = new Date(currentDate.setDate(last));

                    items = items.filter(e => {

                        let pass = false;
                        let dStart = null;
                        let dEnd = null;

                        if (e.StartPlan !== null) {
                            dStart = new Date(e.StartPlan);

                            if (e.StartFact !== null)
                                dStart = new Date(e.StartPlan);
                        }

                        if (e.EndPlan !== null) {
                            dEnd = new Date(e.EndPlan);

                            if (e.EndFact !== null)
                                dEnd = new Date(e.EndFact);
                        }

                        if (dStart !== null && dEnd !== null)
                            pass = (dStart <= firstday && dEnd >= firstday) || (dStart >= firstday && dStart <= lastday) ;

                        return pass;
                    });
                }

                // фильтр по статусу
                if (this.filters.status.length > 0)
                    items = items.filter(e => this.filters.status.indexOf(e.ProjectTaskStatusId) > -1);

                // фильтр по проекту
                if (this.filters.projects.length > 0)
                    items = items.filter(e => this.filters.projects.indexOf(e.ProjectId) > -1);

                // фильтр по группе
                if (this.filters.groups.length > 0)
                    items = items.filter(e => this.filters.groups.indexOf(e.GroupId) > -1);

                // фильтр по исполнителю
                if (this.filters.performers.length > 0)
                    items = items.filter(e => {

                        let users = e.Users.split(',');
                        let pass = false;

                        for (let user of users) {
                            if (this.filters.performers.indexOf(Number.parseInt(user)) > -1) {
                                pass = true;
                                break;
                            }
                        }

                        return pass;
                    });

                // поиск
                items = items.filter(e => this.searchItem(e));

                return items;
            }
        },
        methods: {
            fetchData(uri, callback) {

                let self = this;

                return fetch(uri, {
                    credentials: 'include',
                    accept: 'application/json'
                })
                    .then(response => {
                        if (response.status !== 200) {
                            console.log(response)
                            throw new Error(response.status + " - " + response.statusText)
                        }
                        else
                            return response.json()
                    })
                    .then(data => {
                        callback(data);
                        return data;
                    })
                    .catch(ex => {
                        console.log(uri);
                        console.log(ex);
                        self.errorMessage = ex;
                        self.snackbar = true;
                    });
            },
            async loadTasks() {
                let self = this;
                this.loadings.users = true;

                let uri = this.api;

                if (this.filters.myTask) {
                    uri += '/my';
                }

                await this.fetchData(uri, (data) => {
                    self.items = data;
                    self.loadings.users = false;
                });

            },
            async loadItems() {

                this.loadings.firstLoad = true;

                let self = this;

                await this.fetchData('/api/projects', (data) => {
                    self.projects = data;
                });

                await this.fetchData('/api/groups', (data) => {
                    self.groups = data;
                });

                await this.fetchData('/api/projectTaskStatuses', (data) => {
                    self.statuses = data;
                    self.filters.status = [self.statuses[0].ProjectTaskStatusId];
                });

                await this.fetchData('/api/users', (data) => {
                    self.users = data;
                });

                await this.loadTasks();

                this.loadings.firstLoad = false;
            },
            loadTaskTypes(projectId) {

                this.loadings.taskTypes = true;

                let self = this;

                this.fetchData('/api/projectTaskTypes/p/' + projectId, (data) => {
                    self.taskTypes = data;
                    self.loadings.taskTypes = false;
                });
            },
            async loadPerformers(projectTaskId) {
                let self = this;

                this.editedItem.Users = [];
                this.loadings.performers = true;

                await this.fetchData('/api/projectTaskPerformers/task/' + projectTaskId, (data) => {
                    self.editedItem.Users = data.map(e => e.UserId);
                    self.loadings.performers = false;
                });

            },
            editItem(item) {
                this.editedIndex = this.items.indexOf(item);
                this.editedItem = Object.assign({}, item);

                if (this.editedItem.ProjectId > 0 && this.editedItem.ProjectTaskTypeId > 0) {
                    this.loadTaskTypes(this.editedItem.ProjectId);
                }

                if (this.editedItem.ProjectTaskId > 0) {
                    this.loadPerformers(this.editedItem.ProjectTaskId);
                }

                this.dialog = true;
            },
            async deleteItem() {

                if (!confirm("Удалить выбранную запись?")) return;

                let self = this;
                let item = this.editedItem;

                await fetch(this.api + '/' + item.ProjectTaskId, {
                    method: 'DELETE',
                    credentials: 'include',
                })
                    .then(async (response) => {

                        if (response.ok) {
                            let index = this.items.indexOf(item);
                            this.items.splice(index, 1);
                            this.close();
                        }
                        else {
                            if (response.status === 400) {
                                self.errorMessage = await response.text();
                                self.snackbar = true;
                            }
                            else if (response.status === 404) {
                                self.errorMessage = "Запись не найдена";
                                self.snackbar = true;
                            }
                        }

                    });
            },
            close() {
                this.dialog = false
                this.taskTypes = [];
                this.$nextTick(() => {
                    this.editedItem = Object.assign({}, this.defaultItem)
                    this.editedIndex = -1
                })
            },
            async save() {

                this.loadings.save = true;

                let self = this;
                let needClose = false;
                let method = 'POST';
                const formData = new FormData();

                formData.append('ProjectId', this.editedItem.ProjectId);
                formData.append('ProjectTaskStatusId', this.editedItem.ProjectTaskStatusId);
                formData.append('ProjectTaskTypeId', this.editedItem.ProjectTaskTypeId);
                formData.append('GroupId', this.editedItem.GroupId);

                formData.append('StartPlan', this.toISOString(this.editedItem.StartPlan));
                formData.append('StartFact', this.toISOString(this.editedItem.StartFact));

                formData.append('EndPlan', this.toISOString(this.editedItem.EndPlan));
                formData.append('EndFact', this.toISOString(this.editedItem.EndFact));

                formData.append('Details', this.editedItem.Details);
                formData.append('Note', this.editedItem.Note);
                formData.append('DurationHours', this.editedItem.DurationHours);
                formData.append('Priority', this.editedItem.Priority);

                formData.append('Users', this.editedItem.Users);

                if (this.editedIndex > -1) {
                    method = 'PUT';
                    formData.append('ProjectTaskId', this.editedItem.ProjectTaskId);
                }

                await fetch(this.api, {
                    method: method,
                    credentials: 'include',
                    body: formData
                })
                    .then(async (response) => {

                        self.loadings.save = false;

                        if (response.ok) {

                            const item = await response.json();

                            if (this.editedIndex > -1)
                                Object.assign(self.items[this.editedIndex], item);
                            else
                                self.items.push(item);

                            needClose = true;
                        }
                        else {
                            if (response.status === 400) {
                                self.errorMessage = await response.text();
                                self.snackbar = true;
                            }
                        }

                    });

                if (needClose)
                    this.close()
            },
            closeSnackbar() {
                this.snackbar = false;
                this.errorMessage = '';
            },
            toISOString(v) {
                if (v == undefined || v == null || v === '')
                    return '';

                return new Date(Date.parse(v)).toISOString();
            },
            abbreviation(t) {
                return t.split(' ').map(e => e[0]).join('').toUpperCase();
            },
            getDetailsString(label, login, dateChanged) {

                if (login !== null)
                    return label + this.localeFormat(dateChanged) + ' (' + login + ')';
                else
                    return label + ' - нет информации';

            },
            localeFormat(d) {

                if (d === undefined || d === null || d === '')
                    return '';

                return new Date(Date.parse(d)).toLocaleDateString();
            },
            sortFormat(d) {

                if (d === undefined || d === null || d === '')
                    return '';

                return new Date(Date.parse(d)).toISOString().split('T')[0];
            },
            searchItem(e) {

                if (this.search === null || this.search === '')
                    return true;

                if (e.Details.toLocaleLowerCase().includes(this.search.toLocaleLowerCase()))
                    return true;

                if (e.ProjectName.toLocaleLowerCase().includes(this.search.toLocaleLowerCase()))
                    return true;

                if (e.Performers.toLocaleLowerCase().includes(this.search.toLocaleLowerCase()))
                    return true;

                if (e.Group.GroupName.toLocaleLowerCase().includes(this.search.toLocaleLowerCase()))
                    return true;

                return false;
            },
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
            onRowClick(e) {
                this.editItem(e);
            },
            onChangeMyTask() {
                this.filters.myTask = !this.filters.myTask;
                this.loadTasks();
            },
            onChangeFilterWeek() {
                this.filters.week = !this.filters.week;
            }
        },
        components: {
            datePicker
        }
    };
</script>

<style>
    .col {
        padding: .15rem !important;
    }

    .toolbar-filters .v-input {
        height: 48px;
    }

    .table-tasks .v-data-table-header th {
        padding: 0 10px !important;
    }
</style>