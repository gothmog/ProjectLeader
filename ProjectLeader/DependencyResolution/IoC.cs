// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoC.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using ProjectLeader.Service;
using ProjectLeader.Service.Iface;
using StructureMap;
using StructureMap.Graph;
namespace ProjectLeader.DependencyResolution {
    public static class IoC {
        public static IContainer Initialize() {
            ObjectFactory.Initialize(x =>
                        {
                            x.Scan(scan =>
                                    {
                                        scan.TheCallingAssembly();
                                        scan.AssemblyContainingType(typeof(MongoBaseRepository.IMongoRepository));
                                        scan.WithDefaultConventions();
                                    });
                            x.For<MongoBaseRepository.IMongoRepository>().Use<MongoBaseRepository.MongoRepository>().Ctor<string>("db").Is("Projekty");
                            x.For<IHornbachParser>().Use<HornbachParser>();
                            x.For<ITaskService>().Use<TaskService>();
                        });
            return ObjectFactory.Container;
        }
    }
}