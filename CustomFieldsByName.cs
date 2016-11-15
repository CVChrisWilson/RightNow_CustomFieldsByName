        public void setUpdateFlag(IGlobalContext globContext, IContact contactRecord, string menuFieldName, string menuFieldItem)
        {
            IOptlistItem _updateCustomerFlag = globContext.GetOptlist(92).First(x => string.Equals(x.Label, menuFieldName, StringComparison.InvariantCultureIgnoreCase));
            IList<IOptlistItem> _updateCustomerFlagItems = globContext.GetOptlist((int)_updateCustomerFlag.ID + (0x3 << 24));

            foreach (IOptlistItem Flag in _updateCustomerFlagItems)
            {
                if (string.Equals(Flag.Label, menuFieldItem, StringComparison.InvariantCultureIgnoreCase))
                {
                    contactRecord.CustomField.First(x => x.CfId == _updateCustomerFlag.ID).ValInt = Flag.ID;
                }
            }
        }

        public List<Tuple<string, UpdateStatus>> getUpdateFlags(IGlobalContext globContext, IContact contactRecord, List<string> updateFlagNames)
        {
            List<Tuple<string, UpdateStatus>> updateStatusList = new List<Tuple<string, UpdateStatus>>();

            // get all custom fields that have a name which matches the names in the passed in list
            IList<IOptlistItem> _updateFlags = globContext.GetOptlist(92).Where(x => (updateFlagNames.Where(y => 
            string.Equals(y, x.Label, StringComparison.InvariantCultureIgnoreCase)).ToList().Count > 0)).ToList();

            // foreach custom field that we found
            foreach (IOptlistItem UpdateFlag in _updateFlags)
            {
                // get all menu items for each flag
                IList<IOptlistItem> _updateFlagItems = globContext.GetOptlist((int)UpdateFlag.ID + (0x3 << 24));
                //MessageBox.Show("updateflagitems: " + _updateFlagItems[0].Label);
                // if menu item id is equal to selected id
                //x.id = menu field item id
                //UpdateFlag.ID = custom field id
                foreach (ICfVal customField in contactRecord.CustomField)
                {
                    if (customField.CfId == UpdateFlag.ID)
                    {
                        foreach (IOptlistItem _updateFlag in _updateFlagItems)
                        {
                            if (_updateFlag.ID == customField.ValInt)
                            {
                                try
                                {
                                    if (string.Equals(_updateFlag.Label, "Called", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        updateStatusList.Add(Tuple.Create(UpdateFlag.Label, UpdateStatus.Called));
                                    }
                                    if (string.Equals(_updateFlag.Label, "NotCalled", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        updateStatusList.Add(Tuple.Create(UpdateFlag.Label, UpdateStatus.NotCalled));
                                    }
                                    if (string.Equals(_updateFlag.Label, "Failure", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        updateStatusList.Add(Tuple.Create(UpdateFlag.Label, UpdateStatus.Failure));
                                    }
                                    if (string.Equals(_updateFlag.Label, "Success", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        updateStatusList.Add(Tuple.Create(UpdateFlag.Label, UpdateStatus.Succeeded));
                                    }
                                }
                                catch (NullReferenceException ex)
                                {
                                    updateStatusList.Add(Tuple.Create(UpdateFlag.Label, UpdateStatus.NotCalled));
                                }
                                //MessageBox.Show("Value for " + UpdateFlag.Label + " is " + _updateFlag.Label);
                            }
                        }
                    }
                }
            }
            return updateStatusList;
        }