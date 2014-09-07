using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Odyssey.Engine;
using Odyssey.Graphics.Organization;
using Odyssey.Graphics.Organization.Commands;
using Odyssey.Graphics.Shaders;
using Odyssey.Organization.Commands;
using Odyssey.Utilities.Extensions;
using SharpDX;
using SharpDX.Direct3D11;

namespace Odyssey.Graphics
{
    public class StateViewer
    {
        private readonly IServiceRegistry services;
        private readonly DirectXDevice device;
        PreferredRasterizerState rasterizerState;
        PreferredBlendState blendState;
        PreferredDepthStencilState depthStencilState;
        private readonly LinkedList<Command> sourceCommands;
        private readonly LinkedList<Command> resultCommands;
        private LinkedListNode<Command> cursor;
        private LinkedListNode<Command> resultCursor;


        public StateViewer(IServiceRegistry services, IEnumerable<Command> list)
        {
            Contract.Requires<ArgumentNullException>(list != null, "List cannot be null.");
            this.services = services;
            device = services.GetService<IOdysseyDeviceService>().DirectXDevice;
            resultCommands = new LinkedList<Command>();
            sourceCommands = new LinkedList<Command>(list);
        }

        void AddNewCommand(Command command)
        {
            resultCursor=resultCommands.AddLast(command);
        }

        void SaveState(Technique technique)
        {
            PreferredRasterizerState preferredRasterizerState = technique.Mapping.Key.RasterizerState;
            PreferredBlendState preferredBlendState = technique.Mapping.Key.BlendState;
            PreferredDepthStencilState preferredDepthStencilState = technique.Mapping.Key.DepthStencilState;

            if (CheckRasterizerState(technique))
                AddNewCommand(new RasterizerStateChangeCommand(services, device.RasterizerStates[preferredRasterizerState.ToString()]));

            if (CheckBlendState(technique))
                AddNewCommand(new BlendStateChangeCommand(services, device.BlendStates[preferredBlendState.ToString()]));

            if (CheckDepthStencilState(technique))
                AddNewCommand(new DepthStencilStateChangeCommand(services, device.DepthStencilStates[preferredDepthStencilState.ToString()]));

            rasterizerState = preferredRasterizerState;
            blendState = preferredBlendState;
            depthStencilState = preferredDepthStencilState;
        }

        void CheckState(Command command)
        {
            var cRender = command as ITechniqueRenderCommand;
            if (cRender == null)
            {
                AddNewCommand(command);
                return;
            }

            Technique technique = cRender.Technique;

            SaveState(technique);
            AddNewCommand(command);
        }


        public bool CheckBlendState(Technique technique)
        {
            return technique.Mapping.Key.BlendState != blendState;
        }

        public bool CheckRasterizerState(Technique technique)
        {
            return technique.Mapping.Key.RasterizerState != rasterizerState;
        }

        public bool CheckDepthStencilState(Technique technique)
        {
            return technique.Mapping.Key.DepthStencilState != depthStencilState;
        }

        public LinkedList<Command> Analyze()
        {
            if (!sourceCommands.Any())
                return null;

            cursor = sourceCommands.First;


            while (cursor != null)
            {
                CheckState(cursor.Value);
                cursor = cursor.Next;
            }
            
            // Go back to first ITechniqueRenderCommand
            var firstRenderCommand = (ITechniqueRenderCommand)sourceCommands.First(c=> c is ITechniqueRenderCommand);
            SaveState(firstRenderCommand.Technique);
            return resultCommands;
        }


    }
}
