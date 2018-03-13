using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JohnLambe.Util.StateMachine;

namespace JohnLambe.Tests.JLUtilsTest.StateMachine
{
    [TestClass]
    public class StateMachineTest
    {
        [TestMethod]
        public void SimpleStateTransitionTest()
        {
            var stateMachine = new StateTransitionValidator<int, object>(100);
            stateMachine.Transitions = new StateTransitionValidator<int, object>.StateTransition[]
            {
                new StateTransitionValidator<int, object>.StateTransition(100,101),
                new StateTransitionValidator<int, object>.StateTransition(100,102),
                new StateTransitionValidator<int, object>.StateTransition(102,100),
                new StateTransitionValidator<int, object>.StateTransition(102,103),
                new StateTransitionValidator<int, object>.StateTransition(103,104),
                new StateTransitionValidator<int, object>.StateTransition(104,102),
                new StateTransitionValidator<int, object>.StateTransition(104,102),   // duplicate
            };

            Assert.AreEqual(100, stateMachine.State);

            stateMachine.State = 102;
            Assert.AreEqual(102, stateMachine.State);

            stateMachine.State = 103;
            Assert.AreEqual(103, stateMachine.State);

            stateMachine.State = 104;
            Assert.AreEqual(104, stateMachine.State);

            stateMachine.State = 102;
            Assert.AreEqual(102, stateMachine.State);

            stateMachine.State = 100;
            Assert.AreEqual(100, stateMachine.State);

            TestUtil.AssertThrows<InvalidOperationException>(() => stateMachine.State = 103);
            TestUtil.AssertThrows<InvalidOperationException>(() => stateMachine.State = 90);

            stateMachine.State = 101;
            Assert.AreEqual(101, stateMachine.State);

            TestUtil.AssertThrows<InvalidOperationException>(() => stateMachine.State = 100);
        }

        public enum StateType
        {
            State_A,
            State_B,
            State_C,
            State_D
        }

        [TestMethod]
        public void StateTransitionWithGuardCondition()
        {
            var stateMachine = new StateTransitionValidator<StateType, object>(StateType.State_B);

            stateMachine.Transitions = new StateTransitionValidator<StateType, object>.StateTransition[]
            {
                new StateTransitionValidator<StateType, object>.StateTransition(StateType.State_A, StateType.State_B, model => (model as int?) == 100),
                new StateTransitionValidator<StateType, object>.StateTransition(StateType.State_A, StateType.State_B),
                new StateTransitionValidator<StateType, object>.StateTransition(StateType.State_A, StateType.State_C, model => model?.Equals("A") ?? false ),
                StateTransitionValidator<StateType,object>.StateTransition.CreateFromAny(StateType.State_D),
                new StateTransitionValidator<StateType, object>.StateTransition(StateType.State_B, StateType.State_A, model => true),
            };

            stateMachine.State = StateType.State_A;
            Assert.AreEqual(StateType.State_A, stateMachine.State);

            stateMachine.State = StateType.State_B;    // two transitions; second one is valid
            Assert.AreEqual(StateType.State_B, stateMachine.State);

            stateMachine.State = StateType.State_A;
            Assert.AreEqual(StateType.State_A, stateMachine.State);

            TestUtil.AssertThrows<InvalidOperationException>(() => stateMachine.State = StateType.State_C);              // Guard condition fails

            stateMachine.Model = "A";
            stateMachine.State = StateType.State_C;                        // same condition now passes
            Assert.AreEqual(StateType.State_C, stateMachine.State);

            TestUtil.AssertThrows<InvalidOperationException>(() => stateMachine.State = StateType.State_A);
        }

        public void Invariants()
        { 
            var stateMachine = new StateTransitionValidator<State<StateMachineConsumer>, StateMachineConsumer>();

            /*
            stateMachine.Transitions = new StateTransitionValidator<int, object>.StateTransition[]
            {
                new StateTransitionValidator<int, object>.StateTransition(100,101),
                new StateTransitionValidator<int, object>.StateTransition(100,102),
                new StateTransitionValidator<int, object>.StateTransition(102,100),
                new StateTransitionValidator<int, object>.StateTransition(102,103),
            };

            Assert.AreEqual(100, stateMachine.State);

            stateMachine.State = 102;
            Assert.AreEqual(102, stateMachine.State);

            stateMachine.State = 100;
            Assert.AreEqual(100, stateMachine.State);

            TestUtil.AssertThrows<InvalidOperationException>(() => stateMachine.State = 103);
            TestUtil.AssertThrows<InvalidOperationException>(() => stateMachine.State = 90);

            stateMachine.State = 101;
            Assert.AreEqual(101, stateMachine.State);
            */
        }



    }

    public class StateMachineConsumer
    {
        private int Property { get; set; }

        //        protected static readonly State State1 = new State( (newState) => newState != State2 && Property != 0);
        //        protected static readonly State State2 = new State((newState) => newState != State2);
        protected static readonly State<StateMachineConsumer> State3 = new State<StateMachineConsumer>(null, (model, newState) => model.Property != 0);
        protected static readonly State<StateMachineConsumer> State4 = new State<StateMachineConsumer>(null, (model, newState) => newState == State3);
    }

}
