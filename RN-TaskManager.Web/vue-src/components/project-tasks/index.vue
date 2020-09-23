<template>
    <div class="component-container">

        <v-toolbar flat color="white">
            <v-toolbar-title>{{title}}</v-toolbar-title>

            <v-divider class="mx-4"
                       inset
                       vertical></v-divider>

                <v-btn @click="onChangeMyTask" class="mb-2 mr-2">
                    <v-icon small
                            :color="myTask ? 'green' : 'grey'"
                            class="mr-2">
                        mdi-brightness-1
                    </v-icon>
                    Мои задачи
                </v-btn>

                <v-text-field v-model="search"
                              class="mt-4"
                              label="Поиск"
                              outlined
                              dense
                              clearable></v-text-field>

            <v-spacer></v-spacer>
            <v-dialog v-model="dialog" max-width="800px">
                <template v-slot:activator="{ on, attrs }">
                    <v-btn color="primary"
                           dark
                           class="mb-2"
                           v-bind="attrs"
                           v-on="on">Добавить запись</v-btn>
                </template>
                <v-card>
                    <v-card-title>
                        <span class="headline">Карточка задачи</span>
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

                        </v-container>
                    </v-card-text>

                    <v-card-actions>
                        <v-btn color="red darken-1" text @click="deleteItem">Удалить</v-btn>

                        <v-spacer></v-spacer>
                        <v-btn color="blue darken-1" text @click="close">Отмена</v-btn>
                        <v-btn color="blue darken-1" text @click="save">Сохранить</v-btn>
                    </v-card-actions>
                </v-card>
            </v-dialog>
        </v-toolbar>

        <v-data-table :headers="headers"
                      :items="tasks"
                      :loading="loading"
                      dense="true"
                      hide-default-footer="true"
                      @click:row="onRowClick"
                      items-per-page="500">

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
                { text: '', value: 'TaskStatusName', width: '50px', sortable: false },

                { text: 'Проект', value: 'ProjectName' },
                { text: 'Тип задачи', value: 'TaskTypeName' },
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
                StartPlan: new Date(Date.now()).toISOString(),
                EndPlan: '',
                StartFact: '',
                EndFact: '',
                DurationHours: 0,
                Priority: 0,
                Users: [],
            },
            loading: false,
            myTask: true,
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
                return this.items.filter(e => this.searchItem(e));
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
                this.loading = true;

                let uri = this.api;

                if (this.myTask) {
                    uri += '/my';
                }

                await this.fetchData(uri, (data) => {
                    self.items = data;
                });

                this.loading = false;
            },
            async loadItems() {

                let self = this;

                await this.loadTasks();

                await this.fetchData('/api/projects', (data) => {
                    self.projects = data;
                });

                await this.fetchData('/api/groups', (data) => {
                    self.groups = data;
                });

                await this.fetchData('/api/projectTaskStatuses', (data) => {
                    self.statuses = data;
                });

                this.fetchData('/api/users', (data) => {
                    self.users = data;
                });
            },
            loadTaskTypes(projectId) {
                let self = this;

                this.fetchData('/api/projectTaskTypes/p/' + projectId, (data) => {
                    self.taskTypes = data;
                });
            },
            loadPerformers(projectTaskId) {
                let self = this;

                self.editedItem.Users = [];

                this.fetchData('/api/projectTaskPerformers/task/' + projectTaskId, (data) => {
                    self.editedItem.Users = data.map(e => e.UserId);
                });
            },
            editItem(item) {
                this.editedIndex = this.items.indexOf(item);
                this.editedItem = Object.assign({}, item);

                if (this.editedItem.ProjectId > 0 && this.editedItem.ProjectTaskTypeId > 0) {
                    this.loadTaskTypes(this.editedItem.ProjectId);
                }
                console.log(this.editedItem.ProjectTaskId);

                if (this.editedItem.ProjectTaskId > 0) {
                    console.log('пытаеся загрузить исполнителей');
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
                formData.append('DurationHours', this.editedItem.DurationHours);
                formData.append('Priority', this.editedItem.Priority);

                formData.append('Users', this.editedItem.Users);

                if (this.editedIndex > -1) {
                    method = 'PUT';
                    formData.append('ProjectTaskId', this.editedItem.ProjectTaskStatusId);
                }

                await fetch(this.api, {
                    method: method,
                    credentials: 'include',
                    body: formData
                })
                    .then(async (response) => {

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
            localeFormat(d) {

                if (d === undefined || d === null || d === '')
                    return '';

                return new Date(Date.parse(d)).toLocaleDateString();
            },
            searchItem(e) {

                if (this.search === null || this.search === '')
                    return true;

                if (e.Details.toLocaleLowerCase().includes(this.search.toLocaleLowerCase()))
                    return true;

                if (e.ProjectName.toLocaleLowerCase().includes(this.search.toLocaleLowerCase()))
                    return true;

                return false;
            },
            onRowClick(e) {
                this.editItem(e);
            },
            onChangeMyTask() {
                this.myTask = !this.myTask;
                this.loadTasks();
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

    /*    .v-text-field {
        padding-top:0;
    }*/


</style>