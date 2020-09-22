<template>
    <div class="">

        <v-divider></v-divider>

        <v-dialog v-model="dialog" max-width="500px">
            <template v-slot:activator="{ on, attrs }">
                <v-btn color="teal"
                       dark
                       class="mt-2 mb-2"
                       v-bind="attrs"
                       small
                       v-on="on">Добавить тип работ</v-btn>
            </template>
            <v-card>
                <v-card-title>
                    <span class="headline">Карточка записи</span>
                </v-card-title>

                <v-card-text>
                    <v-container>
                        <v-row>
                            <v-col cols="12">
                                <v-text-field v-model="editedItem.ProjectTaskTypeName" label="Наименование типа работ"></v-text-field>
                            </v-col>
                        </v-row>
                    </v-container>
                </v-card-text>

                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="blue darken-1" text @click="close">Отмена</v-btn>
                    <v-btn color="blue darken-1" text @click="save">Сохранить</v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
       

        <v-data-table :headers="headers"
                      :items="items"
                      :loading="loading"
                      dense="true"
                      hide-default-footer="true"
                      items-per-page="500">


            <template v-slot:item.actions="{ item }">
                <div class="d-flex">
                    <v-spacer></v-spacer>
                    <v-icon small
                            class="mr-2"
                            @click="editItem(item)">
                        mdi-pencil
                    </v-icon>
                    <v-icon small
                            @click="deleteItem(item)">
                        mdi-delete
                    </v-icon>
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
    export default {
        props: ['ProjectId'],
        data: () => ({
            dialog: false,
            snackbar: false,
            errorMessage: '',
            items: [],
            headers: [
                { text: 'Наименование типа работ', value: 'ProjectTaskTypeName' },
                { text: '', value: 'actions', sortable: false },
            ],
            editedIndex: -1,
            editedItem: {
                ProjectTaskTypeId: 0,
                ProjectTaskTypeName: '',
                Order: 1,
            },
            defaultItem: {
                ProjectTaskTypeId: 0,
                ProjectTaskTypeName: '',
                Order: 1,
            },
            loading: false,
            api: '/api/projectTaskTypes'
        }),
        created() {
            this.loadItems()
        },
        computed: {
            allowCreate: function () { return this.ProjectId > 0}
        },
        watch: {
            dialog(val) {
                val || this.close()
            },
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
            async loadItems() {

                let self = this;
                this.loading = true;

                await this.fetchData(this.api + '/p/' + this.ProjectId, (data) => {
                    self.items = data;
                });

                this.loading = false;
            },
            editItem(item) {
                this.editedIndex = this.items.indexOf(item)
                this.editedItem = Object.assign({}, item)
                this.dialog = true
            },
            async deleteItem(item) {

                if (!confirm("Удалить выбранную запись?")) return;

                let self = this;

                await fetch(this.api + '/' + item.ProjectTaskTypeId, {
                    method: 'DELETE',
                    credentials: 'include',
                })
                    .then(async (response) => {

                        if (response.ok) {
                            let index = this.items.indexOf(item);
                            this.items.splice(index, 1);
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

                formData.append('ProjectId', this.ProjectId);
                formData.append('ProjectTaskTypeName', this.editedItem.ProjectTaskTypeName);

                if (this.editedIndex > -1) {
                    method = 'PUT';
                    formData.append('ProjectTaskTypeId', this.editedItem.ProjectTaskTypeId);
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
        }
    };
</script>

<style>
</style>